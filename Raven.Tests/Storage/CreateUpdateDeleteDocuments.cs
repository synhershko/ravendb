using System;
using Newtonsoft.Json.Linq;
using Raven.Database;
using Xunit;

namespace Raven.Tests.Storage
{
    public class CreateUpdateDeleteDocuments : AbstractDocumentStorageTest, IDisposable
    {
        private readonly DocumentDatabase db;

        public CreateUpdateDeleteDocuments()
        {
            db = new DocumentDatabase("divan.db.test.esent");
        }

        #region IDisposable Members

        public void Dispose()
        {
            db.Dispose();
        }

        #endregion

        [Fact]
        public void When_creating_document_with_id_specified_will_return_specified_id()
        {
            var documentId = db.Put("1", Guid.Empty, JObject.Parse("{ first_name: 'ayende', last_name: 'rahien'}"),
                                    new JObject());
            Assert.Equal("1", documentId);
        }

        [Fact]
        public void When_creating_document_with_no_id_specified_will_return_guid_as_id()
        {
            var documentId = db.Put(null, Guid.Empty, JObject.Parse("{ first_name: 'ayende', last_name: 'rahien'}"),
                                    new JObject());
            Assert.DoesNotThrow(() => new Guid(documentId));
        }

        [Fact]
        public void When_creating_documents_with_no_id_specified_will_return_guids_in_sequencal_order()
        {
            var documentId1 = db.Put(null, Guid.Empty, JObject.Parse("{ first_name: 'ayende', last_name: 'rahien'}"),
                                     new JObject());
            var documentId2 = db.Put(null, Guid.Empty, JObject.Parse("{ first_name: 'ayende', last_name: 'rahien'}"),
                                     new JObject());
            Assert.Equal(1, new Guid(documentId2).CompareTo(new Guid(documentId1)));
        }

        [Fact]
        public void Can_create_and_read_document()
        {
            db.Put("1", Guid.Empty, JObject.Parse("{  first_name: 'ayende', last_name: 'rahien'}"), new JObject());
            var document = db.Get("1").ToJson();

            Assert.Equal("ayende", document.Value<string>("first_name"));
            Assert.Equal("rahien", document.Value<string>("last_name"));
        }

        [Fact]
        public void Can_edit_document()
        {
            db.Put("1", Guid.Empty, JObject.Parse("{ first_name: 'ayende', last_name: 'rahien'}"), new JObject());

            db.Put("1", db.Get("1").Etag, JObject.Parse("{ first_name: 'ayende2', last_name: 'rahien2'}"), new JObject());
            var document = db.Get("1").ToJson();

            Assert.Equal("ayende2", document.Value<string>("first_name"));
            Assert.Equal("rahien2", document.Value<string>("last_name"));
        }

        [Fact]
        public void Can_delete_document()
        {
            db.Put("1", Guid.Empty, JObject.Parse("{ first_name: 'ayende', last_name: 'rahien'}"), new JObject());
            var document = db.Get("1");
            db.Delete("1", document.Etag);

            Assert.Null(db.Get("1"));
        }

        [Fact]
        public void Can_query_document_by_id_when_having_multiple_documents()
        {
            db.Put("1", Guid.Empty, JObject.Parse("{ first_name: 'ayende', last_name: 'rahien'}"), new JObject());
            db.Put("21", Guid.Empty, JObject.Parse("{ first_name: 'ayende2', last_name: 'rahien2'}"), new JObject());
            var document = db.Get("21").ToJson();

            Assert.Equal("ayende2", document.Value<string>("first_name"));
            Assert.Equal("rahien2", document.Value<string>("last_name"));
        }

        [Fact]
        public void Querying_by_non_existant_document_returns_null()
        {
            Assert.Null(db.Get("1"));
        }
    }
}