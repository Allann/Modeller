﻿using Humanizer;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hy.Modeller.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Module
    {
        private string _company;

        [JsonProperty]
        public string Company
        {
            get => _company;
            set => _company = value.Dehumanize().Pascalize();
        }

        [JsonProperty]
        public Name Project { get; set; } = new Name("");

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Name Feature { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultSchema { get; set; }

        [JsonProperty]
        public List<Model> Models { get; } = new List<Model>();

        public string Namespace => string.IsNullOrEmpty(Project.Singular.StaticVariable) ? Company :
                    Feature != null ? $"{Company}.{Project.Singular.StaticVariable}.{Feature.Singular.StaticVariable}" :
                    $"{Company}.{Project.Singular.StaticVariable}";
    }
}