using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace PassKitSharp.Tests
{
    [TestFixture]
    public class PKParseUnitTests
    {
        [Test]
        public void ParseSamplePasses()
        {
            var passes = System.IO.Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SamplePasses"), "*.pkpass");

            foreach (var file in passes)
            {
                var pass = PassKit.Parse(file);
            }

        }
    }
}
