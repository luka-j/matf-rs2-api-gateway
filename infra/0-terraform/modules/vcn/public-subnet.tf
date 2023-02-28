resource "oci_core_subnet" "vcn_public_subnet" {

  # Required
  compartment_id = "${var.compartment_id}"
  vcn_id = "${module.vcn.vcn_id}"
  cidr_block = "10.0.0.0/24"
 
  # Optional
  route_table_id = "${module.vcn.ig_route_id}"
  security_list_ids = [oci_core_security_list.public_security_list.id]
  display_name = "${var.name}-public-subnet"
}
