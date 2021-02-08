using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BasicEnvironmentTests
    {
        private BasicEnvironment basicEnviroment = new BasicEnvironment(1, 0.1f,0,0);

        // A Test behaves as an ordinary method
        [Test]
        public void ConsentationDecreasesAsDistIncreasesTest()
        {
            float c1 = basicEnviroment.getConsentration(0,0);
            float c2 = basicEnviroment.getConsentration(5, 5);
            Assert.Greater(c1, c2);
            // Use the Assert class to test conditions
        }
        
        [Test]
        public void MaxCIsOneTest()
        {
            float c = basicEnviroment.getConsentration(0, 0);
            Assert.AreEqual(1f, c);
        }
    }
}
