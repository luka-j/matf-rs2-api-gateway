output "compartment_id" {
  value = oci_identity_compartment.tf_compartment.id
}

output "availability_domains" {
  value = data.oci_identity_availability_domains.ads.availability_domains
}
