terraform {
  required_providers {
    oci = {
      source = "oracle/oci"
      version = ">= 4.102.0"
    }
  }
}

module "vcn" {
  source  = "oracle-terraform-modules/vcn/oci"
  version = "3.5.3"
  compartment_id = "${var.compartment_id}"

  vcn_name = "${var.name}"
  vcn_dns_label = "${var.dns_label}"
  vcn_cidrs = ["10.0.0.0/16"]
  
  create_internet_gateway = true
  create_nat_gateway = true
  create_service_gateway = true  
}
