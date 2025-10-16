# module "ai_search_cocktails_index_simple" {
#   source = "git::ssh://git@github.com/mtnvencenzo/Terraform-Modules.git//modules/ai-search-cosmos-index-simple"

#   azureai_account_id                  = var.ai_search_service_id
#   search_index_reader_role_assignment = var.search_index_reader_role_assignment

#   cosmosdb_account_id   = var.cosmosdb_account_id
#   cosmos_database_name  = var.cosmos_database_name
#   cosmos_container_name = var.cosmos_container_name

#   providers = {
#     restapi = restapi
#   }

#   tags = var.tags

#   # https://learn.microsoft.com/en-us/rest/api/searchservice/create-data-source
#   cosmos_datasource_json = jsonencode({
#     "name" : "${var.datasource_name}",
#     "description" : null,
#     "type" : "cosmosdb",
#     "credentials" : {
#       "connectionString" : "ResourceId=${var.cosmosdb_account_id};Database=${var.cosmos_database_name};IdentityAuthType=AccessToken"
#     },
#     "container" : {
#       "name" : var.cosmos_container_name,
#       "query" : "SELECT * FROM c WHERE c._ts >= @HighWaterMark ORDER BY c._ts"
#     },
#     "dataChangeDetectionPolicy" : {
#       "@odata.type" : "#Microsoft.Azure.Search.HighWaterMarkChangeDetectionPolicy",
#       "highWaterMarkColumnName" : "_ts"
#     },
#     "dataDeletionDetectionPolicy" : null,
#     "encryptionKey" : null
#   })

#   # https://learn.microsoft.com/en-us/rest/api/searchservice/create-indexer
#   cosmos_standard_lucene_indexer_json = jsonencode({
#     "name" : "${var.indexer_name}",
#     "description" : null,
#     "dataSourceName" : "${var.datasource_name}",
#     "skillsetName" : null,
#     "targetIndexName" : "${var.index_name}",
#     "disabled" : null,
#     "schedule" : {
#       "interval" : "P1D",
#       "startTime" : "2025-06-06T12:23:48.423Z"
#     },
#     "parameters" : null,
#     "fieldMappings" : [
#       {
#         "sourceFieldName" : "Title",
#         "targetFieldName" : "TitlePrefix",
#         "mappingFunction" : null
#       },
#       {
#         "sourceFieldName" : "Title",
#         "targetFieldName" : "TitleSuffix",
#         "mappingFunction" : null
#       },
#       {
#         "sourceFieldName" : "Title",
#         "targetFieldName" : "Title",
#         "mappingFunction" : null
#       }
#     ],
#     "outputFieldMappings" : [],
#     "encryptionKey" : null
#   })

#   # To do https://learn.microsoft.com/en-us/rest/api/searchservice/create-index
#   cosmos_standard_lucene_index_json = jsonencode({
#     "name" : "${var.index_name}",
#     "defaultScoringProfile" : "${var.index_scoring_profile_name}",
#     "fields" : [
#       {
#         "name" : "id",
#         "type" : "Edm.String",
#         "searchable" : false,
#         "filterable" : false,
#         "retrievable" : true,
#         "sortable" : false,
#         "facetable" : false,
#         "key" : true,
#         "synonymMaps" : []
#       },
#       {
#         "name" : "Title",
#         "type" : "Edm.String",
#         "searchable" : true,
#         "filterable" : false,
#         "retrievable" : true,
#         "sortable" : false,
#         "facetable" : false,
#         "key" : false,
#         "analyzer" : "standard.lucene",
#         "synonymMaps" : []
#       },
#       {
#         "name" : "TitlePrefix",
#         "type" : "Edm.String",
#         "searchable" : true,
#         "filterable" : false,
#         "retrievable" : false,
#         "sortable" : false,
#         "facetable" : false,
#         "key" : false,
#         "indexAnalyzer" : "ngram_front_analyzer",
#         "searchAnalyzer" : "keyword",
#         "synonymMaps" : []
#       },
#       {
#         "name" : "TitleSuffix",
#         "type" : "Edm.String",
#         "searchable" : true,
#         "filterable" : false,
#         "retrievable" : false,
#         "sortable" : false,
#         "facetable" : false,
#         "key" : false,
#         "indexAnalyzer" : "ngram_back_analyzer",
#         "searchAnalyzer" : "keyword",
#         "synonymMaps" : []
#       },
#       {
#         "name" : "IsIba",
#         "type" : "Edm.Boolean",
#         "searchable" : false,
#         "filterable" : false,
#         "retrievable" : true,
#         "sortable" : false,
#         "facetable" : false,
#         "key" : false,
#         "synonymMaps" : []
#       },
#       {
#         "name" : "eras",
#         "type" : "Collection(Edm.String)",
#         "searchable" : true,
#         "filterable" : false,
#         "retrievable" : false,
#         "sortable" : false,
#         "facetable" : false,
#         "key" : false,
#         "analyzer" : "standard.lucene",
#         "synonymMaps" : []
#       },
#       {
#         "name" : "Description",
#         "type" : "Edm.String",
#         "searchable" : true,
#         "filterable" : false,
#         "retrievable" : true,
#         "sortable" : false,
#         "facetable" : false,
#         "key" : false,
#         "analyzer" : "standard.lucene",
#         "synonymMaps" : []
#       },
#       {
#         "name" : "DescriptiveTitle",
#         "type" : "Edm.String",
#         "searchable" : true,
#         "filterable" : false,
#         "retrievable" : true,
#         "sortable" : false,
#         "facetable" : false,
#         "key" : false,
#         "analyzer" : "standard.lucene",
#         "synonymMaps" : []
#       },
#       {
#         "name" : "Ingredients",
#         "type" : "Collection(Edm.ComplexType)",
#         "fields" : [
#           {
#             "name" : "Name",
#             "type" : "Edm.String",
#             "searchable" : true,
#             "filterable" : false,
#             "retrievable" : true,
#             "sortable" : false,
#             "facetable" : false,
#             "key" : false,
#             "analyzer" : "standard.lucene",
#             "synonymMaps" : []
#           },
#           {
#             "name" : "Units",
#             "type" : "Edm.Double",
#             "searchable" : false,
#             "filterable" : false,
#             "retrievable" : true,
#             "sortable" : false,
#             "facetable" : false,
#             "key" : false,
#             "synonymMaps" : []
#           },
#           {
#             "name" : "UoM",
#             "type" : "Edm.String",
#             "searchable" : false,
#             "filterable" : false,
#             "retrievable" : true,
#             "sortable" : false,
#             "facetable" : false,
#             "key" : false,
#             "synonymMaps" : []
#           }
#         ]
#       }
#     ],
#     "scoringProfiles" : [
#       {
#         "name" : "${var.index_scoring_profile_name}",
#         "text" : {
#           "weights" : {
#             "TitlePrefix" : 10,
#             "TitleSuffix" : 10,
#             "Title" : 10,
#             "DescriptiveTitle" : 3,
#             "Ingredients/Name" : 3,
#             "Description" : 1,
#           }
#         },
#         "functions" : []
#       }
#     ],
#     "suggesters" : [
#       {
#         "name" : "${var.index_suggester_name}",
#         "searchMode" : "analyzingInfixMatching",
#         "sourceFields" : [
#           "Title",
#           "Description",
#           "DescriptiveTitle",
#           "Ingredients/Name"
#         ]
#       }
#     ],
#     "analyzers" : [
#       {
#         "@odata.type" : "#Microsoft.Azure.Search.CustomAnalyzer",
#         "name" : "ngram_front_analyzer",
#         "charFilters" : [],
#         "tokenizer" : "keyword_v2",
#         "tokenFilters" : ["lowercase", "front_edgeNGram"]
#       },
#       {
#         "@odata.type" : "#Microsoft.Azure.Search.CustomAnalyzer",
#         "name" : "ngram_back_analyzer",
#         "charFilters" : [],
#         "tokenizer" : "keyword_v2",
#         "tokenFilters" : ["lowercase", "back_edgeNGram"]
#       }
#     ],
#     "tokenizers" : [],
#     "tokenFilters" : [
#       {
#         "@odata.type" : "#Microsoft.Azure.Search.EdgeNGramTokenFilterV2",
#         "name" : "front_edgeNGram",
#         "minGram" : 2,
#         "maxGram" : 25,
#         "side" : "front"
#       },
#       {
#         "@odata.type" : "#Microsoft.Azure.Search.EdgeNGramTokenFilterV2",
#         "name" : "back_edgeNGram",
#         "minGram" : 2,
#         "maxGram" : 25,
#         "side" : "back"
#       }
#     ],
#     "charFilters" : [],
#     "similarity" : {
#       "@odata.type" : "#Microsoft.Azure.Search.BM25Similarity"
#     }
#   })
# }