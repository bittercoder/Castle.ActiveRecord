// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord.Framework.Config
{
	using System;
	using System.Configuration;
	using System.Xml;

	/// <summary>
	/// Reads the configuration from a entry 'activerecord'
	/// in the xml associated with the AppDomain
	/// </summary>
	public class ActiveRecordSectionHandler : XmlConfigurationSource, IConfigurationSectionHandler
	{
		/// <summary>
		/// Creates a configuration section handler.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext">Configuration context object.</param>
		/// <param name="section"></param>
		/// <returns>The created section handler object.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			FixupThreadInfoType(section);

			PopulateSource(section);

			return this;
		}

		static void FixupThreadInfoType(XmlNode section)
		{
			const string oldHybridWebThreadScopeInfo = "Castle.ActiveRecord.Framework.Scopes.HybridWebThreadScopeInfo, Castle.ActiveRecord";
			const string newHybridWebThreadScopeInfo = "Castle.ActiveRecord.Framework.Scopes.HybridWebThreadScopeInfo, Castle.ActiveRecord.Web";

			var threadInfoTypeAttr = section.Attributes["threadinfotype"];

			if (threadInfoTypeAttr == null) return;

			string value = threadInfoTypeAttr.Value;

			if (value.Contains(oldHybridWebThreadScopeInfo)
					&& !(value.Contains(newHybridWebThreadScopeInfo)))
			{
				threadInfoTypeAttr.Value = value.Replace(oldHybridWebThreadScopeInfo, newHybridWebThreadScopeInfo);	
			}
		}

		/// <summary>
		/// Gets the sole instance.
		/// </summary>
		/// <value>The instance.</value>
		public static IConfigurationSource Instance
		{
			get
			{
				IConfigurationSource source;

				source =
					ConfigurationManager.GetSection("activerecord") as IConfigurationSource;

				if (source == null)
				{
					String message = "Could not obtain configuration from the AppDomain config file." +
					                 " Sorry, but you have to fill the configuration or provide a " +
					                 "IConfigurationSource instance yourself.";

					throw new ConfigurationErrorsException(message);
				}

				return source;
			}
		}

		/// <summary>
		/// Returns the sole instance through a factory method for use with 
		/// Spring.Net (see AR-ISSUE-213)
		/// </summary>
		/// <returns>the sole instance</returns>
		public static IConfigurationSource GetInstance()
		{
			return Instance;
		}
	}
}
