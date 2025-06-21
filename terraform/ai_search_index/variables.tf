variable "ai_search_service_host_name" {
  type = string
}

variable "ai_search_service_id" {
  type = string
}

variable "ai_search_service_key" {
  type = string
}

variable "cosmosdb_account_id" {
  type = string
}

variable "cosmos_database_name" {
  type = string
}

variable "cosmos_container_name" {
  type = string
}

variable "tags" {
  type    = map(string)
  default = {}
}

variable "datasource_name" {
  type    = string
  default = "ds-cosmos-cocktails"
}

variable "indexer_name" {
  type    = string
  default = "idxer-cosmos-cocktails-standard-lucene"
}


variable "index_name" {
  type    = string
  default = "idx-cosmos-cocktails-standard-lucene"
}

variable "index_scoring_profile_name" {
  type    = string
  default = "sp-weighted-profile-1"
}

variable "index_suggester_name" {
  type    = string
  default = "ac-suggestor-1"
}

variable "search_index_reader_role_assignment" {
  type    = list(string)
  default = []
}
