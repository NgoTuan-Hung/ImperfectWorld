// using UnityEngine;
// using UnityEngine.InputSystem;

// public class TestObjectPoolRevamp : MonoBehaviour
// {
//     ObjectPool objectPool;
//     public GameObject arrowPrefab;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         objectPool = new(
//             arrowPrefab,
//             new PoolArgument(ComponentType.PoolRevampPoolObject, PoolArgument.WhereComponent.Self)
//         );
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Keyboard.current.kKey.isPressed)
//         {
//             var poolObject = objectPool.PickOne();
//             poolObject.gameObject.transform.position = Vector3.zero;
//         }
//     }
// }
