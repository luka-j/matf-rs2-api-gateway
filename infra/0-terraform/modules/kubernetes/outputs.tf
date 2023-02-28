output "cluster_name" {
  value = oci_containerengine_cluster.oke_cluster.name
}
output "cluster_ocid" {
  value = oci_containerengine_cluster.oke_cluster.id
}
output "cluster_kubernetes_version" {
  value = oci_containerengine_cluster.oke_cluster.kubernetes_version
}
output "cluster_state" {
  value = oci_containerengine_cluster.oke_cluster.state
}

# Outputs for k8s node pool

output "node_pool_name" {
  value = oci_containerengine_node_pool.oke_node_pool.name
}
output "node_pool_ocid" {
  value = oci_containerengine_node_pool.oke_node_pool.id
}
output "node_pool_kubernetes_version" {
  value = oci_containerengine_node_pool.oke_node_pool.kubernetes_version
}
output "node_size" {
  value = oci_containerengine_node_pool.oke_node_pool.node_config_details[0].size
}
output "node_shape" {
  value = oci_containerengine_node_pool.oke_node_pool.node_shape
}
