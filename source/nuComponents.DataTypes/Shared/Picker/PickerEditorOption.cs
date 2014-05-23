﻿
namespace nuComponents.DataTypes.Shared.Picker
{
    using Newtonsoft.Json;

    /// <summary>
    /// would like to rename this to DataSourceEditorItem (consequence = lots of changes)
    /// </summary>
    public class PickerEditorOption
    {
        [JsonProperty("key")]
        public string Key { get; set; }
     
        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
