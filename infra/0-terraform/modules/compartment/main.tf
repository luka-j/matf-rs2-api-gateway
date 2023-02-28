terraform {
  required_providers {
    oci = {
      source = "oracle/oci"
      version = ">= 4.102.0"
    }
  }
}

resource "oci_identity_compartment" "tf_compartment" {
    compartment_id = "${var.tenancy_id}"
    description = "${var.description}"
    name = "${var.name}"
}


data "oci_identity_availability_domains" "ads" {
  compartment_id = "${var.tenancy_id}"
}
