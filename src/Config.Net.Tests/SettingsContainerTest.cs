﻿using System;
using System.Collections.Generic;
using Config.Net.Stores;
using Xunit;

namespace Config.Net.Tests
{
   public class AllSettings : SettingsContainer
   {
      public Option<string> AuthClientId { get; } = new Option<string>("AuthenticationClientId", "default id");

      public Option<string> AuthClientSecret { get; } = new Option<string>();

      protected override void OnConfigure(IConfigConfiguration configuration)
      {
         configuration.CacheTimeout = TimeSpan.FromMinutes(1);

         //configuration.UseAzureTable("storage_account_name", "storage_key", "table_name", "application_name");
      }
   }

   public class SettingsContainerTest
   {
      private InMemoryConfigStore _store;

      public SettingsContainerTest()
      {
         _store = new InMemoryConfigStore();
      }

      class MyContainer : SettingsContainer
      {
         #region crap
         private IConfigStore _store;
         private ITypeParser _customParser;

         public MyContainer(IConfigStore store, ITypeParser customParser = null) : base("MyApp")
         {
            _store = store;
            _customParser = customParser;
         }
         #endregion

         public Option<TimeSpan> StrongSpan { get; } = new Option<TimeSpan>(TimeSpan.FromDays(1));

         public Option<int> Timeout { get; } = new Option<int>(2);

         public Option<int> NoInitTimeout { get; set; }

         public Option<MyType> MyTypeField { get; set; }

         protected override void OnConfigure(IConfigConfiguration configuration)
         {
            configuration.AddStore(_store);

            if (_customParser != null) configuration.AddParser(_customParser);
         }
      }

      [Fact]
      public void Read_IntegerTimeout_Reads()
      {
         var c = new MyContainer(_store);

         int timeout = c.Timeout;

         Assert.Equal(2, timeout);
      }

      [Fact]
      public void Read_NoInit_Acceptable()
      {
         var c = new MyContainer(_store);

         Assert.Equal("MyApp.NoInitTimeout", c.NoInitTimeout.Name);
      }

      [Fact]
      public void Read_ByGetProperty_SameResult()
      {
         var c = new MyContainer(_store);
         var value = c.NoInitTimeout.Value;
      }

      [Fact]
      public void CustomTypes_ForMyOwnClass_ParsesOut()
      {
         var c = new MyContainer(_store, new MyTypeParser());
         _store.Write("MyApp.MyTypeField", "anything");  //add a value to framework doesn't return default value but invokes custom type parser

         MyType mt = c.MyTypeField;

         Assert.Equal("constant S", mt.S);
      }

      class MyType
      {
         public string S;
      }

      class MyTypeParser : ITypeParser
      {
         public IEnumerable<Type> SupportedTypes
         {
            get
            {
               return new[] { typeof(MyType) };
            }
         }

         public string ToRawString(object value)
         {
            return null;
         }

         public bool TryParse(string value, Type t, out object result)
         {
            result = new MyType { S = "constant S" };

            return true;
         }
      }

      public void Demo()
      {
         var c = new AllSettings();

         string clientId = c.AuthClientId;
         string clientSecret = c.AuthClientSecret;

         c.AuthClientId.Write("new value");
      }

      private class LambdaModule
      {
         
      }
   }
}
