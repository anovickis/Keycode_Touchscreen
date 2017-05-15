using System;
using System.Xml;

namespace XmlConfig
{
	/// <summary>
    /// AUTHOR: Neil Taylor
    /// 
	/// DESIGN/PURPOSE: 
    /// This class is used in order to rip apart and access different nodes in an
    /// XML document without having to know the specific order which they are listed in.
    /// </summary>
	public class XmlConfigDoc 
	{
		public XmlDocument xmlDoc {get; private set;}

        /// <summary>
        /// Default Constructor
        /// </summary>
		public XmlConfigDoc()
		{
			xmlDoc = new XmlDocument();
		}

        /// <summary>
        /// Secondary Constructor4
        /// </summary>
        /// <param name="filename">Name of the file to be analyzed</param>
		public XmlConfigDoc( string filename ) : base ()
		{
			xmlDoc = new XmlDocument();
			xmlDoc.Load( filename );
		}

        /// <summary>
        /// This method provides a list of all of the nodes within the XML document
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns>List of nodes</returns>
		public XmlConfigNodeList GetNodes( string xpath )
		{
			XmlNodeList nodes = xmlDoc.SelectNodes( xpath );
			return ( new XmlConfigNodeList( nodes ) );
		}

        /// <summary>
        /// This method provides a single node defined by 'xpath'
        /// </summary>
        /// <param name="xpath">name of the node to be extracted</param>
        /// <returns>the chosen node</returns>
        public XmlConfigNode GetNode(string xpath)
        {
            return (new XmlConfigNode(xmlDoc.SelectSingleNode(xpath)));
        }

        public XmlConfigNode GetNode(string xpath, XmlNamespaceManager manager)
        {
            return (new XmlConfigNode(xmlDoc.SelectSingleNode(xpath,manager)));
        }
	}

    /// <summary>
    /// Auxiliary inner class used to manage lists of XML nodes
    /// </summary>
	public class XmlConfigNodeList 
	{
		XmlNodeList nodeList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeList">generic list of nodes</param>
		public XmlConfigNodeList( XmlNodeList nodeList )
		{
			this.nodeList = nodeList;
		}

        /// <summary>
        /// Array handler to access nodes in the node list
        /// </summary>
        /// <param name="index">Array index</param>
        /// <returns>Indexed node</returns>
		public XmlConfigNode this[ int index ]
		{
			get
			{
				return ( new XmlConfigNode( nodeList[index] ) );
			}
		}

        /// <summary>
        /// Returns the number of nodes in the list.
        /// </summary>
		public int Count
		{
			get
			{
				return nodeList.Count;
			}
		}
	}

    /// <summary>
    /// Auxiliary class which defines a single XML node
    /// </summary>
	public class XmlConfigNode 
	{
		XmlNode node;

		public XmlConfigNode( XmlNode node )
		{
			this.node = node;
		}

		public string GetValue( string xpath )
		{
			XmlNode x = node.SelectSingleNode( xpath );

			if( x == null)
				return ( "" );

			switch ( x.NodeType )
			{
				case XmlNodeType.Attribute:
				case XmlNodeType.Element:
					return ( x.FirstChild.Value.Trim() );
				

				default:
					return ( "" );
			}
		}

        public string GetValue(string xpath, XmlNamespaceManager manager)
        {
            XmlNode x = node.SelectSingleNode(xpath,manager);

            if (x == null)
                return ("");

            switch (x.NodeType)
            {
                case XmlNodeType.Attribute:
                case XmlNodeType.Element:
                    return (x.FirstChild.Value.Trim());


                default:
                    return ("");
            }
        }

        /// <summary>
        /// Gives you the ability to set the value on an element. Be sure to
        /// save the file before moving on so you dont loose the changes.
        /// </summary>
        /// <param name="xpath">name of the node</param>
        /// <param name="value">value which you want to store in the node</param>
        /// <returns>returns the value if successful. otherwise returns empty string</returns>
        public string SetValue(string xpath, string value)
        {
            XmlNode x = node.SelectSingleNode(xpath);

            if (x == null)
                return ("");

            switch (x.NodeType)
            {
                case XmlNodeType.Attribute:
                case XmlNodeType.Element:
                    return (x.FirstChild.Value = value);


                default:
                    return ("");
            }
                
        }

		public string GetValue()
		{
			return ( node.FirstChild.Value.Trim() );
		}

		public string GetName()
		{
			return ( node.Name );
		}

		public long GetNumber( string xpath )
		{
			return ( GetNumber( xpath, 0 ) );
		}

		/// <summary>
		///     See if the element defined by 'xPath' exists in the document
		/// </summary>
		/// <param name="xpath" type="string">
		///     <para>
		///         the xpath to search for
		///     </para>
		/// </param>
		/// <returns>
		///     true if at least one matching element is found
		/// </returns>
		public bool isDefined( string xpath )
		{
			if ( node.SelectSingleNode( xpath ) != null )
				return ( true );
			else
				return ( false );
		}

		/// <summary>
		///     Retreive a number from the XML config document. If the value specified by the "xpath" 
		///     argument is not found then return the default value (defValue) instead.
		/// </summary>
		/// <param name="xpath" type="string">
		///     <para>
		///         The xpath to the value to retrieve from the XML document
		///     </para>
		/// </param>
		/// <param name="defValue" type="long">
		///     <para>
		///         If the vlaue is not found then return this value
		///     </para>
		/// </param>
		/// <returns>
		///     A long value...
		/// </returns>
		public long GetNumber( string xpath, long defValue )
		{
			string strValue = GetValue( xpath );

			if(strValue == "")
				return ( defValue );
			else
				return Int64.Parse( strValue );
		}

		public XmlConfigNode GetNode( string xpath )
		{
			return ( new XmlConfigNode( node.SelectSingleNode( xpath ) ) );
		}

		public XmlConfigNode GetParent( )
		{
			return ( new XmlConfigNode( node.ParentNode ) );
		}

		public XmlConfigNodeList GetNodes( string xpath )
		{
			XmlNodeList nodes = node.SelectNodes( xpath );
			return ( new XmlConfigNodeList( nodes ) );
		}
	}
}
