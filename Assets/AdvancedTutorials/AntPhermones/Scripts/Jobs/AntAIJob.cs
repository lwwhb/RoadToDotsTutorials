using DOTS.DOD;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.ADVANCED.ANTPHERMONES
{
    // 随机行走
    [WithAll(typeof(Ant))]
    [BurstCompile]
    partial struct RandomSteeringJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]public RefRW<RandomSingleton> random;
        void Execute(ref Direction direction, in AntProperties properties)
        {
            float steeringStrength = properties.blobData.Value.randomSteering;
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
                
                int offset = mapSize*mapSize*colonyID.id;
                if (testX >= 0 && testY >= 0 && testX < mapSize && testY < mapSize)
                {
                    var index = (int)testX + (int)testY * mapSize;
                    var value = pheromones[index+offset].strength;
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
        public void Execute(Entity entity, ref Ant ant, ref Position position, in Direction direction)
        {
            int output = 0; 

            var directionInRadians = direction.direction / 180f * (float) math.PI;

            // this for loop makes us check the direction * -1 and * 1
            for (int i = -1; i <= 1; i += 2)
            {
                float angle = directionInRadians + i * math.PI * 0.25f;
                float testX = position.position.x + math.cos(angle) * distance;
                float testY = position.position.y + math.sin(angle) * distance;

                float obstacleX, obstacleY;
                if (DetectPositionInBuckets(testX, testY, buckets, obstacleSize, mapSize, bucketResolution, out obstacleX, out obstacleY))
                {
                    output -= i;

                    // Move the ant away from the obstacle 
                    var dx = position.position.x - obstacleX;
                    var dy = position.position.y - obstacleY;
                    var pushbackAngle = math.atan2(dy, dx);

                    position.position.x += math.cos(pushbackAngle) * wallPushbackUnits;
                    position.position.y += math.sin(pushbackAngle) * wallPushbackUnits;
                }
            }

            ant.wallSteering = output * steeringStrength / (float) math.PI * 180f;
        }
        
        public static bool DetectPositionInBuckets(float x, float y, in NativeArray<Bucket> buckets, float obstacleSize, float mapSize, int bucketResolution, out float obstacleX, out float obstacleY )
        {
            obstacleX = 0;
            obstacleY = 0;
            // test map boundaries
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
                    if (math.pow(x - obstacleX, 2) + math.pow(y - obstacleY, 2) <= math.pow(obstacleSize,2))
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
        public float distance;
        public float mapSize;
        public float obstacleSize;
        public float steeringStrength;
        
        public int bucketResolution;
        public float2 resourcePosition;
        public float2 homePosition;
        [ReadOnly]
        public NativeArray<Bucket> buckets;

        public void Execute(ref Ant ant, in Position position, in Direction direction)
        {
            float2 targetPosition = ant.hasResource ? homePosition : resourcePosition;

            float dx = targetPosition.x - position.position.x;
            float dy = targetPosition.y - position.position.y;
            float dist = math.sqrt(dx * dx + dy * dy);


            // we are at the target
            if (dist < 4f)
            {
                ant.hasResource = !ant.hasResource;
                ant.resourceSteering = 180f;
                return;
            }


            int stepCount = (int)math.ceil(dist * .5f);
            bool blocked = false;
            for (int i = 0; i < stepCount; ++i)
            {
                float t = (float)i / stepCount;
                float _, __;
                if (ObstacleDetectionJob.DetectPositionInBuckets(position.position.x + dx * t, position.position.y + dy * t,
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
}
