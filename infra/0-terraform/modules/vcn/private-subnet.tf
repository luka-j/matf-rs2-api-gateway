resource "oci_core_subnet" "vcn_private_subnet" {

  # Required
  compartment_id = "${var.compartment_id}"
  vcn_id = "${module.vcn.vcn_id}"
  cidr_block = "10.0.1.0/24"
 
  # Optional
  # Caution: For the route table id, use module.vcn.nat_route_id.
  # Do not use module.vcn.nat_gateway_id, because it is the OCID for the gateway and not the route table.
  route_table_id = "${module.vcn.nat_route_id}"
  security_list_ids = [oci_core_security_list.private_security_list.id]
  display_name = "${var.name}-private-subnet"
}
