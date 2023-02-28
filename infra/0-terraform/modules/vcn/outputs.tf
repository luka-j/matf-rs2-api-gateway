output "vcn_id" {
  description = "OCID of the VCN that is created"
  value = module.vcn.vcn_id
}

output "private_security_list_name" {
  value = oci_core_security_list.private_security_list.display_name
}
output "private_security_list_ocid" {
  value = oci_core_security_list.private_security_list.id
}

output "public_security_list_name" {
  value = oci_core_security_list.public_security_list.display_name
}
output "public_security_list_ocid" {
  value = oci_core_security_list.public_security_list.id
}

output "private_subnet_name" {
  value = oci_core_subnet.vcn_private_subnet.display_name
}
output "private_subnet_ocid" {
  value = oci_core_subnet.vcn_private_subnet.id
}

output "public_subnet_name" {
  value = oci_core_subnet.vcn_public_subnet.display_name
}
output "public_subnet_ocid" {
  value = oci_core_subnet.vcn_public_subnet.id
}
