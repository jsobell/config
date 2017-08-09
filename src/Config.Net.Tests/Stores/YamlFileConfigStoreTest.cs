﻿using Config.Net.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Config.Net.Tests.Stores
{
   public class YamlFileConfigStoreTest : AbstractTestFixture
   {
      private YamlFileConfigStore _yaml;

      public YamlFileConfigStoreTest()
      {
         var testFile = Path.Combine(BuildDir.FullName, "..", "..", "..", "..", "..", "appveyor.yml");
         _yaml = new YamlFileConfigStore(testFile);
      }

      [Fact]
      public void Can_read_simple_property()
      {
         string image = _yaml.Read("image");

         Assert.Equal("Visual Studio 2017", image);
      }
   }
}