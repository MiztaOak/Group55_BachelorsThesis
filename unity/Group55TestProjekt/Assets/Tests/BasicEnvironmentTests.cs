using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BasicEnvironmentTests
    {
        private Environment basicEnviroment = new Environment(1, 0.1f,0,0);

        // A Test behaves as an ordinary method
        [Test]
        public void ConcentationDecreasesAsDistIncreasesTest()
        {
            float c1 = basicEnviroment.getConcentration(0,0);
            float c2 = basicEnviroment.getConcentration(5, 5);
            Assert.Greater(c1, c2);
            // Use the Assert class to test conditions
        }
        
        [Test]
        public void MaxCIsOneTest()
        {
            float c = basicEnviroment.getConcentration(0, 0);
            Assert.AreEqual(1f, c);
        }
    }
}
