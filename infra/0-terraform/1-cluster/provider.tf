terraform {
  required_providers {
    oci = {
      source = "oracle/oci"
      version = "4.112.0"
    }
  }
}

provider "oci" {
  tenancy_ocid = "${var.tenancy_ocid}"
  user_ocid = "${var.user_ocid}"
  private_key_path = "${var.oci_private_key_path}"
  fingerprint = "${var.oci_key_fingerprint}"
  region = "${var.oci_region}"
}
