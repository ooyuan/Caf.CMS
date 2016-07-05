﻿using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Themes;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace CAF.WebSite.Application.WebUI.Theming
{ 
    internal class ThemeVarsRepository
    {
        private static readonly Regex s_keyWhitelist = new Regex(@"^[a-zA-Z0-9_-]+$");
        private static readonly Regex s_valueWhitelist = new Regex(@"^[#@]?[a-zA-Z0-9""' _\.,-]*$");

        public string GetVariablesAsLess(string themeName, int siteId)
        {
            Guard.ArgumentNotEmpty(() => themeName);
            Guard.ArgumentIsPositive(siteId, "siteId");

            var variables = GetLessCssVariables(themeName, siteId);
            var lessCss = TransformToLess(variables);
            return lessCss;
        }

        private IDictionary<string, string> GetLessCssVariables(string themeName, int siteId)
        {
            var result = new Dictionary<string, string>();

			var rawVars = this.GetRawVariables(themeName, siteId);

            foreach (var v in rawVars)
            {
                string key = v.Key;

                if (v.Value == null || !s_keyWhitelist.IsMatch(key))
                    continue;

                string value = v.Value.ToString();

                if (!s_valueWhitelist.IsMatch(value))
                    continue;

                result.Add("var_" + key, value);
            }

            return result;
        }

        internal virtual ExpandoObject GetRawVariables(string themeName, int siteId)
        {
			// we need the Asp.Net here cache in order to define cacheKey dependencies
			var cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("aspnet");

			string cacheKey = ModelCacheEventConsumer.BuildThemeVarsCacheKey(themeName, siteId);
			return cacheManager.Get(cacheKey, () =>
			{
				var themeVarService = EngineContext.Current.Resolve<IThemeVariablesService>();
				var result = themeVarService.GetThemeVariables(themeName, siteId);

				if (result == null)
				{
					return new ExpandoObject();
				}

				return result;
			});
        }

        private string TransformToLess(IDictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var parameter in parameters.Where(kvp => kvp.Value.HasValue()))
            {
                sb.AppendFormat("@{0}: {1};\n", parameter.Key, parameter.Value);
            }

            return sb.ToString();
        }

    }

}
