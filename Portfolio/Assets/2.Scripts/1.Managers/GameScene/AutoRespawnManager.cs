using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Define;

public class AutoRespawnManager : MonoBehaviour
{
    PoolingManager pool;
    SpawnManager spawn;
    Queue<eMonster> _monsterQ;

    int _currAmount = 0;
    int _totalAmount = 0;
    int _reserveAmount = 0;
    [SerializeField] float _spawnRadius = 5.0f;
    [SerializeField] float _spawnTime = 5.0f;


    void Start()
    {
        Init();
        spawn.OnSpawnEvent -= AddMonsterCount;
        spawn.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        while (_reserveAmount + _currAmount < _totalAmount)
        {
            StartCoroutine(ReserveSpawn());
        }
    }

    void Init()
    {
        pool = PoolingManager._pool;
        spawn = SpawnManager._inst;
        _monsterQ = new Queue<eMonster>();
        for (int i = 0; i < pool._poolingUnits.Length; i++)
        {
            if (pool._poolingUnits[i].type != PoolType.Monster)
                continue;

            for (int j = 0; j < pool._poolingUnits[i].amount; j++)
            {
                MonsterCtrl mc = pool._poolingUnits[i].prefab.GetComponent<MonsterCtrl>();
                _monsterQ.Enqueue(mc.mType);
            }
            SetKeepMonsterCount(pool._poolingUnits[i].amount);
        }
    }

    void AddMonsterCount(eMonster type, int value)
    {
        _currAmount += value;
        if (value < 0)
            _monsterQ.Enqueue(type);
    }

    void SetKeepMonsterCount(int value)
    {
        _totalAmount += value;
    }

    IEnumerator ReserveSpawn()
    {
        _reserveAmount++;
        yield return new WaitForSeconds(Random.Range(4, _spawnTime));
        if (_monsterQ.Count == 0)
        {
            _reserveAmount--;
            yield break;
        }
        eMonster type = _monsterQ.Dequeue();
        GameObject go = spawn.Spawn(type);
        MonsterCtrl mc = go.GetComponent<MonsterCtrl>();
        Vector3 randPos = new Vector3();
        if (go.TryGetComponent<NavMeshAgent>(out NavMeshAgent na) == false)
            na = go.AddComponent<NavMeshAgent>();

        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
            randDir.y = 0;
            randPos = go.transform.position + randDir;
            NavMeshPath path = new NavMeshPath();
            if (na.CalculatePath(randPos, path))
                break;
        }
        if (mc.isDead)
            mc.OnResurrectEvent();

        go.transform.position = randPos;
        _reserveAmount--;
    }
}
