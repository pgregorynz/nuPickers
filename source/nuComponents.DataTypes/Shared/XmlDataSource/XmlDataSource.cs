﻿
namespace nuComponents.DataTypes.Shared.XmlDataSource
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.XPath;
    using umbraco;
    using nuComponents.DataTypes.Shared.Picker;

    public class XmlDataSource
    {
        public string XmlSchema { get; set; }

        public string OptionsXPath { get; set; }
        
        public string KeyAttribute { get; set; }
        
        public string LabelAttribute { get; set; }

        public IEnumerable<PickerEditorOption> GetEditorOptions()
        {
            XmlDocument xmlDocument;
            List<PickerEditorOption> editorOptions = new List<PickerEditorOption>();

            switch (this.XmlSchema)
            {
                case "content":
                    xmlDocument = uQuery.GetPublishedXml(uQuery.UmbracoObjectType.Document);
                    break;

                case "media":
                    xmlDocument = uQuery.GetPublishedXml(uQuery.UmbracoObjectType.Media);
                    break;

                case "members":
                    xmlDocument = uQuery.GetPublishedXml(uQuery.UmbracoObjectType.Member);
                    break;

                default:
                    // fallback to expecting path to an xml file ?
                    xmlDocument = null;
                    break;
            }

            if (xmlDocument != null)
            {
                XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
                XPathNodeIterator xPathNodeIterator = xPathNavigator.Select(uQuery.ResolveXPath(this.OptionsXPath));
                List<string> keys = new List<string>(); // used to keep track of keys, so that duplicates aren't added

                string key;
                string markup;

                while (xPathNodeIterator.MoveNext())
                {
                    // media xml is wrapped in a <Media id="-1" /> node to be valid, exclude this from any results
                    // member xml is wrapped in <Members id="-1" /> node to be valid, exclude this from any results
                    // TODO: nuQuery should append something unique to this root wrapper to simplify check here
                    if (xPathNodeIterator.CurrentPosition > 1 ||
                        !(xPathNodeIterator.Current.GetAttribute("id", string.Empty) == "-1" &&
                         (xPathNodeIterator.Current.Name == "Media" || xPathNodeIterator.Current.Name == "Members")))
                    {
                        // check for existance of a key attribute
                        key = xPathNodeIterator.Current.GetAttribute(this.KeyAttribute, string.Empty);

                        // only add item if it has a unique key - failsafe
                        if (!string.IsNullOrWhiteSpace(key) && !keys.Any(x => x == key))
                        {
                            // TODO: ensure key doens't contain any commas (keys are converted saved as csv)
                            keys.Add(key); // add key so that it's not reused

                            // set default markup to use the configured label attribute
                            markup = xPathNodeIterator.Current.GetAttribute(this.LabelAttribute, string.Empty);

                            editorOptions.Add(new PickerEditorOption()
                            {
                                Key = key,
                                Markup = markup
                            });
                        }
                    }
                }
            }

            return editorOptions;
        }
    }
}