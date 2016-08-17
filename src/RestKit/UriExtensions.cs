using System;
using System.Collections.Generic;

namespace RestKit
{
    /// <summary>
    /// Sponsor class for composable helper methods to aid in not having to remember to add a reference to System.ServiceModel.
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Creates a URI template from the provided string value.
        /// </summary>
        /// <param name="uriTemplate">The URI template as a string.</param>
        /// <returns>The <see cref="UriTemplate"/> instance.</returns>
        public static UriTemplate AsUriTemplate(this string uriTemplate)
        {
            return new UriTemplate(uriTemplate);
        }

        /// <summary>
        /// Binds the template parameters by position.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="template">The template.</param>
        /// <param name="values">The values.</param>
        /// <returns>
        /// The fully formed URI.
        /// </returns>
        public static Uri BindTemplateByPosition(this Uri baseUri, UriTemplate template, params string[] values)
        {
            return template.BindByPosition(baseUri, values);
        }

        /// <summary>
        /// Binds the template parameters by name.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="template">The template to bind.</param>
        /// <param name="values">The values to use as key value pairs in a dictionary.</param>
        /// <returns>
        /// The fully formed URI.
        /// </returns>
        public static Uri BindTemplateByName(this Uri baseUri, UriTemplate template, IDictionary<string, string> values)
        {
            return template.BindByName(baseUri, values);
        }
    }
}
