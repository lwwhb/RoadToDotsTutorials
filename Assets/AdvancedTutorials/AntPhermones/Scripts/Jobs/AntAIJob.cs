using DOTS.DOD;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    // 随机行走
    [WithAll(typeof(Ant))]
    [BurstCompile]
    partial struct RandomSteeringJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]public RefRW<RandomSingleton> random;
        void Execute(ref Ant ant, ref Direction direction, in AntProperties properties)
        {
            float steeringStrength = ant.hasResource ? properties.blobData.Value.randomSteering * 0.5f : properties.blobData.Value.randomSteering;
            direction.direction += random.ValueRW.random.NextFloat(-steeringStrength, steeringStrength);
        }
    }

    // 信息素检测
    [BurstCompile]
    [WithAll(typeof(Ant))]
    partial struct PheromoneDetectionJob : IJobEntity
    {
        public int mapSize;
        [ReadOnly]
        public DynamicBuffer<Pheromone> pheromones;

        public void Execute(ref Ant ant, in AntProperties properties, in Position position, in Direction direction, in ColonyID colonyID)
        {
            var output = 0f;
            var directionRadians = direction.direction / 180f * math.PI;
            float steeringStrength = properties.blobData.Value.pheromoneSteerStrength;
            float distance = properties.blobData.Value.pheromoneSteerDistance;
            for (var i=-1;i<=1;i+=2)
            {
                var angle = directionRadians + i * math.PI * 0.25f;
                var testX = position.position.x*mapSize + math.cos(angle) * distance;
                var testY = position.position.y*mapSize + math.sin(angle) * distance;
                
                if (testX >= 0 && testY >= 0 && testX < mapSize && testY < mapSize)
                {
                    var index = (int)testX + (int)testY * mapSize;
                    var value = pheromones[index].strength;
                    output += value * i;
                }
            }
            ant.pheroSteering = math.sign(output) * steeringStrength;
        }
    }

    // 障碍物检测
    [BurstCompile]
    [WithAll(typeof(Ant))]
    partial struct ObstacleDetectionJob : IJobEntity
    {
        public float distance;
        public float mapSize;
        public float obstacleSize;
        public float steeringStrength;
        public int bucketResolution;
        public float wallPushbackUnits;
        [ReadOnly]
        public NativeArray<Bucket> buckets;
        public void Execute(ref Ant ant, ref Position position, in Direction direction)
        {
            int output = 0; 

            var directionInRadians = direction.direction / 180f * (float) math.PI;
            // 循环检查方向的正负
            for (int i = -1; i <= 1; i += 2)
            {
                float angle = directionInRadians + i * math.PI * 0.25f;
                float testX = position.position.x*mapSize + math.cos(angle) * distance;
                float testY = position.position.y*mapSize + math.sin(angle) * distance;
                
                float obstacleX, obstacleY;
                if (DetectPositionInBuckets(testX, testY, buckets, obstacleSize, mapSize, bucketResolution, out obstacleX, out obstacleY))
                {
                    output -= i;

                    // 从障碍物中推回
                    var dx = position.position.x*mapSize - obstacleX;
                    var dy = position.position.y*mapSize - obstacleY;
                    var pushbackAngle = math.atan2(dy, dx);

                    position.position.x += (math.cos(pushbackAngle) * wallPushbackUnits)/mapSize;
                    position.position.y += (math.sin(pushbackAngle) * wallPushbackUnits)/mapSize;
                }
            }

            ant.wallSteering = output * steeringStrength / (float) math.PI * 180f;
        }
        
        public static bool DetectPositionInBuckets(float x, float y, in NativeArray<Bucket> buckets, float obstacleSize, float mapSize, int bucketResolution, out float obstacleX, out float obstacleY )
        {
            obstacleX = 0;
            obstacleY = 0;
            // 测试地图边界
            if (x < 0 || y < 0 || x >= mapSize || y >= mapSize)
            {
                return true;
            }
            else
            {
                int xIndex = (int)(x / mapSize * bucketResolution);
                int yIndex = (int)(y / mapSize * bucketResolution);
                if (xIndex < 0 || yIndex < 0 || xIndex >= bucketResolution || yIndex >= bucketResolution)
                {
                    return false; // ???
                }
                var obstacles = buckets[xIndex + yIndex * bucketResolution];
                foreach (var obstaclePosition in obstacles.obstacles)
                {
                    obstacleX = obstaclePosition.x;
                    obstacleY = obstaclePosition.y;
                    if (math.pow(x - obstacleX*mapSize, 2) + math.pow(y - obstacleY*mapSize, 2) <= math.pow(obstacleSize,2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


    [BurstCompile]
    [WithAll(typeof(Ant))]
    public partial struct ResourceDetectionJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]public RefRW<RandomSingleton> random;
        public float mapSize;
        public float sizeScale;
        public float obstacleSize;
        
        public int bucketResolution;
        [ReadOnly]
        public NativeArray<float2> resourcesPosition;
        [ReadOnly]
        public NativeArray<float2> homesPosition;
        [ReadOnly]
        public NativeArray<Bucket> buckets;

        public void Execute(ref Ant ant, in ColonyID colonyID, in Position position, in Direction direction)
        {
            //int resourcesNum = resourcesPosition.Length;
            //int randomResourceIndex = random.ValueRW.random.NextInt(0, resourcesNum - 1);
            //float2 resourcePosition = resourcesPosition[randomResourceIndex];
            float2 resourcePosition = resourcesPosition[colonyID.id];
            float2 homePosition = homesPosition[colonyID.id];
            float2 targetPosition = ant.hasResource ? homePosition : resourcePosition;

            float dx = targetPosition.x - position.position.x;
            float dy = targetPosition.y - position.position.y;
            float dist = math.sqrt(dx * dx + dy * dy);
            
            // 到达目标点
            if (dist < (ant.hasResource? 8f : 4f)*sizeScale/mapSize)
            {
                ant.hasResource = !ant.hasResource;
                ant.resourceSteering = 180f;
                return;
            }

            int stepCount = (int)math.ceil(dist * mapSize / sizeScale);
            bool blocked = false;
            for (int i = 0; i < stepCount; ++i)
            {
                float t = (float)i / stepCount;
                float _, __;
                if (ObstacleDetectionJob.DetectPositionInBuckets((position.position.x + dx * t)*mapSize, (position.position.y + dy * t)*mapSize,
                    buckets, obstacleSize, mapSize, bucketResolution, out _, out __))
                {
                    blocked = true;
                    break;
                }
            }


            if (blocked)
            {
                ant.resourceSteering = 0;
            }
            else
            {
                float directionInRad = math.radians(direction.direction);

                float targetAngle = math.atan2(targetPosition.y - position.position.y, targetPosition.x - position.position.x);
                
                if (targetAngle - directionInRad > math.PI/2f)
                {
                    ant.resourceSteering = -5f;
                }
                else if (targetAngle - directionInRad < -math.PI/2f)
                {
                    ant.resourceSteering = 5f;
                }
                else
                {
                    ant.resourceSteering = math.degrees(targetAngle - directionInRad)/30f;
                }
            }
        }
    }
    
    
    [BurstCompile]
    [WithAll(typeof(Ant))]
    public partial struct AntTransformJob : IJobEntity
    {
        public void Execute(
            ref Position position, 
            ref Direction direction, 
            ref Speed speed,
            ref LocalTransform localTransform,
            in Ant ant)
        {
            // 计算转向值系数
            // 优先级：资源 > 信息素 > 障碍物
            // 优先级越高，转向角度越大
            // 优先级相同，转向角度相加
            if (ant.resourceSteering > math.EPSILON)
                direction.direction += ant.resourceSteering;
            else
                direction.direction += ant.wallSteering + ant.pheroSteering + ant.resourceSteering;

            while (direction.direction > 180f)
                direction.direction -= 360f;
            
            while (direction.direction < -180f)
                direction.direction += 360f;
            
            // 管理速度
            // 转向时速度减慢
            var steeringInRad = (ant.wallSteering + ant.pheroSteering + ant.resourceSteering) / 180f * math.PI;
            var oldSpeed = speed.speed;
            var targetSpeed = speed.maxSpeed;
            targetSpeed *= 1f - Mathf.Abs(steeringInRad) / 3f;
            speed.speed += (targetSpeed - oldSpeed) * speed.accel;

            var directionRad = direction.direction / 180f * math.PI;
            localTransform.Rotation = quaternion.Euler(0, 0, directionRad);

            // 移动蚂蚁
            var oldPosition = position.position;
            var speedValue = speed.speed;
            var deltaPos = new float2(
                (float)(speedValue * math.cos(directionRad)),
                (float) (speedValue * math.sin(directionRad)));  
            var newPosition = oldPosition + deltaPos;
            
            // 蚂蚁移动出边界，转向180度，否则更新位置
            if (newPosition.x < 0f || newPosition.x > 1.0f || newPosition.y < 0f || newPosition.y > 1.0f)
                direction.direction = direction.direction + 180;
            else
            {
                position.position = newPosition;
                localTransform.Position = new float3(newPosition.x, newPosition.y, 0);
            }
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(Ant))]
    public partial struct PheromoneDropJob : IJobEntity
    {
        public float deltaTime;
        public int mapSize;
        [NativeDisableParallelForRestriction]
        public NativeArray<Pheromone> pheromones;

        public void Execute(in Ant ant, in AntProperties properties, in Position position, in Speed speed)
        {
            var strength = ant.hasResource ? 0.5f : 0.1f;
            strength *= speed.speed / speed.maxSpeed;

            var gridPosition = math.int2(math.floor(position.position*mapSize));
            if (gridPosition.x < 0 || gridPosition.y < 0 || gridPosition.x >= mapSize || gridPosition.y >= mapSize)
            {
                return;
            }

            var index = gridPosition.x + gridPosition.y * mapSize;
            var pheromone = pheromones[index];
            pheromone.strength += properties.blobData.Value.pheromoneGrowthRate * strength * (1f - pheromone.strength) * deltaTime;
            pheromones[index] = pheromone;
        }
    }
    
    [BurstCompile]
    public struct PheromoneDecayJob : IJobFor
    {
        public float pheromoneDecayRate;
        [NativeDisableParallelForRestriction]
        public DynamicBuffer<Pheromone> pheromones;

        public void Execute(int index)
        {
            var pheromone = pheromones[index];
            pheromone.strength *= pheromoneDecayRate;
            pheromones[index] = pheromone;
        }
    }
    
    
    [BurstCompile]
    [WithAll(typeof(Ant))]
    public partial struct AntRenderingJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(in Ant ant, ref URPMaterialPropertyBaseColor color)
        {
            if (ant.hasResource)
                color.Value = new Vector4(1, 1, 0, 1);
            else
                color.Value = new Vector4(0, 0, 1, 1);
        }
    }
}
