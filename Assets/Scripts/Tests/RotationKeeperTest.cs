using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RotationKeeperTest
    {
        
        // Я ПЫТАЛСЯ, ПРАВДА!
        private GameObject obj;

        [SetUp]
        public void Setup()
        {
            obj = Resources.Load("Resources/Cell.prefab") as GameObject;
            Assert.IsNotNull(obj, "obj != null");
                
        }

        [Test]
        public void DemoTestSimplePasses()
        {
//            RotationKeeper rotationKeeper = obj.GetComponent<RotationKeeper>();
      //      rotationKeeper.enabled = false;
            obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0,0 ));
            Debug.Log(obj.transform.localRotation.eulerAngles);
            Assert.IsTrue(obj.transform.localRotation.eulerAngles.x == 0 , "GameObject rotation.x == 0");
       //     rotationKeeper.enabled = true;
            Assert.IsTrue(obj.transform.localRotation.eulerAngles.x == 45, "Rotation Keeper test");
        }
    }
}
