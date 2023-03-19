module "compartment" {
  source = "../modules/compartment"
  tenancy_id = "${var.tenancy_ocid}"
  name = "rs2-api-gateway"
  description = "Compartment for RS2 Project API Gateway"
}

module "vcn" {
  source = "../modules/vcn"
  compartment_id = "${module.compartment.compartment_id}"
  name = "rs2-apigtw-vcn"
  dns_label = "apigtw"
}

module "kubernetes" {
  source = "../modules/kubernetes"
  compartment_id = "${module.compartment.compartment_id}"
  name = "rs2-apigtw-cluster"
  vcn_id = "${module.vcn.vcn_id}"
  service_lb_subnet_id = "${module.vcn.public_subnet_ocid}"
  worker_node_pool_subnet_id = "${module.vcn.private_subnet_ocid}"
  endpoint_subnet_id = "${module.vcn.private_subnet_ocid}"
  availability_domains = module.compartment.availability_domains
  ssh_public_key = "${var.ssh_public_key}"
}
