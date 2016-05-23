using System.Collections.Generic;
using Orchard.Mvc;

namespace Orchard.Settings {
    /// <summary>
    /// Content management functionality to deal with Orchard content items and their parts
    /// </summary>
    public interface IContentManager : IDependency {

        /// <summary>
        /// Instantiates a new content item with the specified type
        /// </summary>
        /// <remarks>
        /// The content item is not yet persisted!
        /// </remarks>
        /// <param name="contentType">The name of the content type</param>
        ContentItem New(string contentType);
        

        /// <summary>
        /// Creates (persists) a new content item
        /// </summary>
        /// <param name="contentItem">The content instance filled with all necessary data</param>
        void Create(ContentItem contentItem);


        /// <summary>
        /// Gets the content item with the specified id
        /// </summary>
        /// <param name="id">Numeric id of the content item</param>
        ContentItem Get(int id);

        /// <summary>
        /// Permanently deletes the specified content item, including all of its content part records.
        /// </summary>
        void Destroy(ContentItem contentItem);

        /// <summary>
        /// Clears the current referenced content items
        /// </summary>
        void Clear();

        /// <summary>
        /// Query for arbitrary content items
        /// </summary>
        IEnumerable<ContentItem> Query(string contentType);

        ContentItemMetadata GetItemMetadata(IContent contentItem);
        IEnumerable<GroupInfo> GetEditorGroupInfos(IContent contentItem);
        IEnumerable<GroupInfo> GetDisplayGroupInfos(IContent contentItem);
        GroupInfo GetEditorGroupInfo(IContent contentItem, string groupInfoId);
        GroupInfo GetDisplayGroupInfo(IContent contentItem, string groupInfoId);

        /// <summary>
        /// Builds the display shape of the specified content item
        /// </summary>
        /// <param name="content">The content item to use</param>
        /// <param name="displayType">The display type (e.g. Summary, Detail) to use</param>
        /// <param name="groupId">Id of the display group (stored in the content item's metadata)</param>
        /// <returns>The display shape</returns>
        dynamic BuildDisplay(IContent content, string displayType = "", string groupId = "");

        /// <summary>
        /// Builds the editor shape of the specified content item
        /// </summary>
        /// <param name="content">The content item to use</param>
        /// <param name="groupId">Id of the editor group (stored in the content item's metadata)</param>
        /// <returns>The editor shape</returns>
        dynamic BuildEditor(IContent content, string groupId = "");

        /// <summary>
        /// Updates the content item and its editor shape with new data through an IUpdateModel
        /// </summary>
        /// <param name="content">The content item to update</param>
        /// <param name="updater">The updater to use for updating</param>
        /// <param name="groupId">Id of the editor group (stored in the content item's metadata)</param>
        /// <returns>The updated editor shape</returns>
        dynamic UpdateEditor(IContent content, IUpdateModel updater, string groupId = "");
    }

    public interface IContentDisplay : IDependency {
        dynamic BuildDisplay(IContent content, string displayType = "", string groupId = "");
        dynamic BuildEditor(IContent content, string groupId = "");
        dynamic UpdateEditor(IContent content, IUpdateModel updater, string groupId = "");
    }
}
