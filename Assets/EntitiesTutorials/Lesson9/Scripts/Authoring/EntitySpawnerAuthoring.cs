using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTS.DOD.LESSON9
{
    public enum BuidingType //建筑类型
    {
        BT_Spawner, //兵营
        BT_DefenderTower, //防御塔
        BT_MAX
    }

    public enum ArmorType //护甲类型
    {
        AT_None = 0, //无甲
        AT_Light, //轻甲
        AT_Normal, //中甲
        AT_Heavy, //重甲
        AT_Hero, //特殊类型甲
        AT_Max
    }

    public enum DamageType //伤害类型
    {
        DT_Slash = 0, //挥砍伤害
        DT_Pricks, //穿刺伤害
        DT_Smash, //粉碎伤害
        DT_Magic, //魔法伤害
        DT_Chaos, //混合型伤害
        DT_Hero, //特殊类伤害
        DT_Max
    }

    struct EntitySpawnerAllComponentData : IComponentData
    {
        public Entity entityProtoType; //生成的entity原型对象，用于实例化克隆          8byte
        public BuidingType buildingType; //建筑类型                                 4byte
        public int level; //当前等级                                                4byte
        public float tickTime; //每多少秒生成一次                                   4byte
        public int spawnCountPerTicktime; //每次生成几个entity                     4byte
        public float maxLife; //最大生命值                                         4byte
        public float currentlife; //当前生命值                                     4byte
        public ArmorType armorType; //护甲类型                                      4byte
        public DamageType damageType; //伤害类型                                    4byte
        public float maxDamage; //最大攻击力                                         4byte
        public float minDamage; //最大攻击力                                         4byte
        public float upgradeTime; //升级时间                                        4byte
        public float upgradeCost; //升级费用                                        4byte

    }

    struct EntitySpawnerComponentData : IComponentData
    {
        public float currentlife; //当前生命值                             4byte
    }

    struct EntitySpawnerBlobData
    {
        public Entity entityProtoType; //生成的entity原型对象，用于实例化克隆       8byte
        public BuidingType buildingType; //建筑类型                              4byte
        public int level; //当前等级                              4byte
        public float tickTime; //每多少秒生成一次                        4byte
        public int spawnCountPerTicktime; //每次生成几个entity                     4byte
        public float maxLife; //最大生命值                             4byte
        public ArmorType armorType; //护甲类型                               4byte
        public DamageType damageType; //伤害类型                               4byte
        public float maxDamage; //最大攻击力                             4byte
        public float minDamage; //最大攻击力                             4byte
        public float upgradeTime; //升级时间                              4byte
        public float upgradeCost; //升级费用                              4byte
    }

    struct EntitySpawnerSettings : IComponentData
    {
        public BlobAssetReference<EntitySpawnerBlobData> blobSettings;          // 8byte
    }

    public class EntitySpawnerAuthoring : MonoBehaviour
    {
        public GameObject protoTypePrefab = null;
        public BuidingType buildingType = BuidingType.BT_Spawner;
        public int level = 1;
        [Range(1.0f, 10.0f)] public float tickTime = 5.0f;
        [Range(1, 8)] public int spawnCountPerTicktime = 1;
        [Range(100, 3000)] public float maxLife = 1000;
        public ArmorType armorType = ArmorType.AT_Normal;
        public DamageType damageType = DamageType.DT_Magic;
        public float maxDamage = 0;
        public float minDamage = 0;
        public float upgradeTime = 100.0f;
        public float upgradeCost = 100;

        public class Baker : Baker<EntitySpawnerAuthoring>
        {
            public override void Bake(EntitySpawnerAuthoring authoring)
            {
                //---使用AllComponentData
                /*var data = new EntitySpawnerAllComponentData
                {  
                    entityProtoType = GetEntity(authoring.protoTypePrefab),
                    buildingType = authoring.buildingType,
                    level = authoring.level,
                    tickTime = authoring.tickTime,
                    spawnCountPerTicktime = authoring.spawnCountPerTicktime,
                    maxLife = authoring.maxLife,
                    armorType = authoring.armorType,
                    damageType = authoring.damageType,
                    maxDamage = authoring.maxDamage,
                    minDamage = authoring.minDamage,
                    upgradeTime = authoring.upgradeTime,
                    upgradeCost = authoring.upgradeCost,
                    currentlife = authoring.maxLife
                };
                AddComponent(data);*/
                //---

                //---使用BlobAssets
                AddComponent(new EntitySpawnerComponentData
                {
                    currentlife = authoring.maxLife
                });

                var settings = CreateSpawnerBlobSettings(authoring);
                AddBlobAsset(ref settings, out var hash);

                AddComponent(new EntitySpawnerSettings
                {
                    blobSettings = settings
                });
                //---
            }

            BlobAssetReference<EntitySpawnerBlobData> CreateSpawnerBlobSettings(EntitySpawnerAuthoring authoring)
            {
                var builder = new BlobBuilder(Allocator.Temp);

                ref EntitySpawnerBlobData spawnerBlobData = ref builder.ConstructRoot<EntitySpawnerBlobData>();

                spawnerBlobData.entityProtoType = GetEntity(authoring.protoTypePrefab);
                spawnerBlobData.buildingType = authoring.buildingType;
                spawnerBlobData.level = authoring.level;
                spawnerBlobData.tickTime = authoring.tickTime;
                spawnerBlobData.spawnCountPerTicktime = authoring.spawnCountPerTicktime;
                spawnerBlobData.maxLife = authoring.maxLife;
                spawnerBlobData.armorType = authoring.armorType;
                spawnerBlobData.damageType = authoring.damageType;
                spawnerBlobData.maxDamage = authoring.maxDamage;
                spawnerBlobData.minDamage = authoring.minDamage;
                spawnerBlobData.upgradeTime = authoring.upgradeTime;
                spawnerBlobData.upgradeCost = authoring.upgradeCost;

                var result = builder.CreateBlobAssetReference<EntitySpawnerBlobData>(Allocator.Persistent);
                builder.Dispose();
                return result;
            }

        }
    }
}
