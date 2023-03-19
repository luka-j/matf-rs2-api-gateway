terraform {
  required_providers {
    oci = {
      source = "oracle/oci"
      version = ">= 4.102.0"
    }
  }
}

resource "oci_containerengine_cluster" "oke_cluster" {
    # Required
    compartment_id = "${var.compartment_id}"
    kubernetes_version = "${var.kubernetes_version}"
    name = "${var.name}"
    vcn_id = "${var.vcn_id}"

    endpoint_config {
      is_public_ip_enabled = false
      subnet_id = "${var.endpoint_subnet_id}"
    }
    
    # Optional
    options {
        add_ons{
            is_kubernetes_dashboard_enabled = false
            is_tiller_enabled = false
        }
        kubernetes_network_config {
            pods_cidr = "10.244.0.0/16"
            services_cidr = "10.96.0.0/16"
        }
        service_lb_subnet_ids = ["${var.service_lb_subnet_id}"]
    }  
}

resource "oci_containerengine_node_pool" "oke_node_pool" {
    # Required
    cluster_id = oci_containerengine_cluster.oke_cluster.id
    compartment_id = "${var.compartment_id}"
    kubernetes_version = "${var.kubernetes_version}"
    name = "${var.name}-pool-a1"
    node_config_details{
        placement_configs{
            availability_domain = var.availability_domains[0].name
            subnet_id = "${var.worker_node_pool_subnet_id}"
        } 
        placement_configs{
            availability_domain = var.availability_domains[1].name
            subnet_id = "${var.worker_node_pool_subnet_id}"
        }
        size = var.a1_pool_size
    }
    node_shape = "VM.Standard.A1.Flex"

    node_shape_config {
        memory_in_gbs = var.a1_pool_memory_gbs
        ocpus = var.a1_pool_ocpus
    }

    # Find image OCID for your region from https://docs.oracle.com/iaas/images/ 
    node_source_details {
         image_id = "ocid1.image.oc1.eu-frankfurt-1.aaaaaaaa6t4srebi4bch25zo2demycy7imnvh4qg36yl24odqrbrl337ccbq"
         source_type = "image"
         boot_volume_size_in_gbs = 50
    }
 
    # Optional
    initial_node_labels {
        key = "name"
        value = "${var.name}"
    }    
}

resource "oci_core_instance" "bastion_host" {
    #Required
    availability_domain = "XiDj:EU-FRANKFURT-1-AD-3" # Micro instances are available only in AD3
    compartment_id = "${var.compartment_id}"
    shape = "VM.Standard.E2.1.Micro"
    display_name = "bastion"
    shape_config {
        memory_in_gbs = 1
        ocpus = 1
    }
    source_details {
        #Required
        source_id = "ocid1.image.oc1.eu-frankfurt-1.aaaaaaaaz7szaviqzpc7ladu7mgm6g3vzuxiuyb2j4vxm3y77ubt7w62ey6q"
        source_type = "image"
        boot_volume_size_in_gbs = 50
    }
    create_vnic_details {
        subnet_id = "${var.service_lb_subnet_id}"
    }
    metadata = {
        ssh_authorized_keys = "${var.ssh_public_key}"
    }
}
