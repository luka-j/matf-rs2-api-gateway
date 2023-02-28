variable "tenancy_id" {
  description = "OCID of the tenancy in which to create a compartment"
  type = string
}

variable "name" {
  description = "Name of the compartment"
  type = string
}

variable "description" {
  description = "Description of the compartment"
  type = string
  default = ""
}
