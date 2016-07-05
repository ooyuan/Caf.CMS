﻿using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Localization;
using CAF.Infrastructure.Core.Utilities;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Localization;
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Domain.Cms.Forums;
using CAF.Infrastructure.Core.Domain.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CAF.WebSite.Application.Services.Seo
{
    public static class SeoExtensions
    {
        #region Article tag

        /// <summary>
        /// Gets article tag SE (search engine) name
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        /// <returns>Article tag SE (search engine) name</returns>
        public static string GetSeName(this ArticleTag articleTag)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return GetSeName(articleTag, workContext.WorkingLanguage.Id);
        }

        /// <summary>
        /// Gets article tag SE (search engine) name
        /// </summary>
        /// <param name="articleTag">Article tag</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Article tag SE (search engine) name</returns>
        public static string GetSeName(this ArticleTag articleTag, int languageId)
        {
            if (articleTag == null)
                throw new ArgumentNullException("articleTag");
            string seName = GetSeName(articleTag.GetLocalized(x => x.Name, languageId));
            return seName;
        }

        #endregion
        #region Forum

        /// <summary>
        /// Gets ForumGroup SE (search engine) name
        /// </summary>
        /// <param name="forumGroup">ForumGroup</param>
        /// <returns>ForumGroup SE (search engine) name</returns>
        public static string GetSeName(this ForumGroup forumGroup)
        {
            if (forumGroup == null)
                throw new ArgumentNullException("forumGroup");
            string seName = GetSeName(forumGroup.Name);
            return seName;
        }

        /// <summary>
        /// Gets Forum SE (search engine) name
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Forum SE (search engine) name</returns>
        public static string GetSeName(this Forum forum)
        {
            if (forum == null)
                throw new ArgumentNullException("forum");
            string seName = GetSeName(forum.Name);
            return seName;
        }

        /// <summary>
        /// Gets ForumTopic SE (search engine) name
        /// </summary>
        /// <param name="forumTopic">ForumTopic</param>
        /// <returns>ForumTopic SE (search engine) name</returns>
        public static string GetSeName(this ForumTopic forumTopic)
        {
            if (forumTopic == null)
                throw new ArgumentNullException("forumTopic");
            string seName = GetSeName(forumTopic.Subject);

            // Trim SE name to avoid URLs that are too long
            var maxLength = 100;
            if (seName.Length > maxLength)
            {
                seName = seName.Substring(0, maxLength);
            }

            return seName;
        }

        #endregion

        #region General

        /// <summary>
        /// Get search engine name
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Search engine name</returns>
        public static string GetSeName<T>(this T entity)
            where T : BaseEntity, ISlugSupported
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return GetSeName(entity, workContext.WorkingLanguage.Id);
        }

        /// <summary>
        ///  Get search engine name
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if language specified one is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Search engine name</returns>
        public static string GetSeName<T>(this T entity, int languageId, bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity, ISlugSupported
        {
            return GetSeName(
                entity,
                languageId,
                EngineContext.Current.Resolve<IUrlRecordService>(),
                EngineContext.Current.Resolve<ILanguageService>(),
                returnDefaultValue,
                ensureTwoPublishedLanguages);
        }

        /// <summary>
        ///  Get search engine name
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if language specified one is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Search engine name</returns>
        public static string GetSeName<T>(this T entity,
            int languageId,
            IUrlRecordService urlRecordService,
            ILanguageService languageService,
            bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity, ISlugSupported
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            string result = string.Empty;
            string entityName = typeof(T).Name;

            if (languageId > 0)
            {
                // ensure that we have at least two published languages
                bool loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var totalPublishedLanguages = languageService.GetLanguagesCount(false);
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }
                // localized value
                if (loadLocalizedValue)
                {
                    result = urlRecordService.GetActiveSlug(entity.Id, entityName, languageId);
                }
            }
            // set default value if required
            if (String.IsNullOrEmpty(result) && returnDefaultValue)
            {
                result = urlRecordService.GetActiveSlug(entity.Id, entityName, 0);
            }

            return result;
        }

        /// <summary>
        /// Validate search engine name
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="seName">Search engine name to validate</param>
        /// <param name="name">User-friendly name used to generate sename</param>
        /// <param name="ensureNotEmpty">Ensreu that sename is not empty</param>
        /// <returns>Valid sename</returns>
        public static string ValidateSeName<T>(this T entity, string seName, string name, bool ensureNotEmpty, int? languageId = null)
             where T : BaseEntity, ISlugSupported
        {
            return entity.ValidateSeName(
                seName,
                name,
                ensureNotEmpty,
                EngineContext.Current.Resolve<IUrlRecordService>(),
                EngineContext.Current.Resolve<SeoSettings>(),
                languageId);
        }

        public static string ValidateSeName<T>(this T entity,
            string seName,
            string name,
            bool ensureNotEmpty,
            IUrlRecordService urlRecordService,
            SeoSettings seoSettings,
            int? languageId = null,
            Func<string, UrlRecord> extraSlugLookup = null)
            where T : BaseEntity, ISlugSupported
        {
            Guard.ArgumentNotNull(() => urlRecordService);
            Guard.ArgumentNotNull(() => seoSettings);

            if (entity == null)
                throw new ArgumentNullException("entity");

            // use name if sename is not specified
            if (String.IsNullOrWhiteSpace(seName) && !String.IsNullOrWhiteSpace(name))
                seName = name;

            // validation
            seName = GetSeName(seName, seoSettings);

            // max length
            seName = seName.Truncate(400);

            if (String.IsNullOrWhiteSpace(seName))
            {
                if (ensureNotEmpty)
                {
                    // use entity identifier as sename if empty
                    seName = entity.Id.ToString();
                }
                else
                {
                    // return. no need for further processing
                    return seName;
                }
            }

            // validate and alter sename if it could be interpreted as SEO code
            if (LocalizationHelper.IsValidCultureCode(seName))
            {
                if (seName.Length == 2)
                {
                    seName = seName + "-0";
                }
            }

            // ensure this sename is not reserved yet
            string entityName = typeof(T).Name;
            int i = 2;
            var tempSeName = seName;

            extraSlugLookup = extraSlugLookup ?? ((s) => null);

            while (true)
            {
                // check whether such slug already exists (and that it's not the current entity)
                var urlRecord = urlRecordService.GetBySlug(tempSeName) ?? extraSlugLookup(tempSeName);
                var reserved1 = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));

                if (!reserved1 && urlRecord != null && languageId.HasValue)
                    reserved1 = (urlRecord.LanguageId != languageId.Value);

                // and it's not in the list of reserved slugs
                var reserved2 = seoSettings.ReservedUrlRecordSlugs.Contains(tempSeName, StringComparer.InvariantCultureIgnoreCase);
                if (!reserved1 && !reserved2)
                    break;

                tempSeName = string.Format("{0}-{1}", seName, i);
                i++;
            }
            seName = tempSeName;

            return seName;
        }


        /// <summary>
        /// Get SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public static string GetSeName(string name)
        {
            var seoSettings = EngineContext.Current.Resolve<SeoSettings>();
            return GetSeName(name, seoSettings);
        }

        /// <summary>
        /// Get SEO friendly name
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="seoSettings">SEO settings</param>
        /// <returns>Result</returns>
        public static string GetSeName(string name, SeoSettings seoSettings)
        {
            return SeoHelper.GetSeName(
                name,
                seoSettings == null ? false : seoSettings.ConvertNonWesternChars,
                seoSettings == null ? false : seoSettings.AllowUnicodeCharsInUrls);
        }

        #endregion
    }
}
