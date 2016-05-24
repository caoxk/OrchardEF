using System.Collections.Generic;
using Orchard.Mvc;

namespace Orchard.DocumentManagement {
    /// <summary>
    /// Content management functionality to deal with Orchard content items and their parts
    /// </summary>
    public interface IDocumentManager : IDependency {

        /// <summary>
        /// Instantiates a new content item with the specified type
        /// </summary>
        /// <remarks>
        /// The content item is not yet persisted!
        /// </remarks>
        /// <param name="contentType">The name of the content type</param>
        DocumentItem New(string contentType);
        

        /// <summary>
        /// Creates (persists) a new content item
        /// </summary>
        /// <param name="contentItem">The content instance filled with all necessary data</param>
        void Create(DocumentItem contentItem);


        /// <summary>
        /// Gets the content item with the specified id
        /// </summary>
        /// <param name="id">Numeric id of the content item</param>
        DocumentItem Get(int id);

        /// <summary>
        /// Permanently deletes the specified content item, including all of its content part records.
        /// </summary>
        void Destroy(DocumentItem contentItem);

        /// <summary>
        /// Clears the current referenced content items
        /// </summary>
        void Clear();

        /// <summary>
        /// Query for arbitrary content items
        /// </summary>
        IEnumerable<DocumentItem> Query(string contentType);

        DocumentItemMetadata GetItemMetadata(IDocument contentItem);
        IEnumerable<GroupInfo> GetEditorGroupInfos(IDocument contentItem);
        IEnumerable<GroupInfo> GetDisplayGroupInfos(IDocument contentItem);
        GroupInfo GetEditorGroupInfo(IDocument contentItem, string groupInfoId);
        GroupInfo GetDisplayGroupInfo(IDocument contentItem, string groupInfoId);

        /// <summary>
        /// Builds the display shape of the specified content item
        /// </summary>
        /// <param name="content">The content item to use</param>
        /// <param name="displayType">The display type (e.g. Summary, Detail) to use</param>
        /// <param name="groupId">Id of the display group (stored in the content item's metadata)</param>
        /// <returns>The display shape</returns>
        dynamic BuildDisplay(IDocument content, string displayType = "", string groupId = "");

        /// <summary>
        /// Builds the editor shape of the specified content item
        /// </summary>
        /// <param name="content">The content item to use</param>
        /// <param name="groupId">Id of the editor group (stored in the content item's metadata)</param>
        /// <returns>The editor shape</returns>
        dynamic BuildEditor(IDocument content, string groupId = "");

        /// <summary>
        /// Updates the content item and its editor shape with new data through an IUpdateModel
        /// </summary>
        /// <param name="content">The content item to update</param>
        /// <param name="updater">The updater to use for updating</param>
        /// <param name="groupId">Id of the editor group (stored in the content item's metadata)</param>
        /// <returns>The updated editor shape</returns>
        dynamic UpdateEditor(IDocument content, IUpdateModel updater, string groupId = "");
    }

    public interface IDocumentDisplay : IDependency {
        dynamic BuildDisplay(IDocument content, string displayType = "", string groupId = "");
        dynamic BuildEditor(IDocument content, string groupId = "");
        dynamic UpdateEditor(IDocument content, IUpdateModel updater, string groupId = "");
    }
}
