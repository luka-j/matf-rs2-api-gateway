import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "react-toastify";

import ccoConfigService, { ccoConfigKey } from "@/services/cco-config-service";

const useDeleteDatasource = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ccoConfigService.deleteDatasource,
    onSuccess: () => {
      queryClient.invalidateQueries([ccoConfigKey]);
      toast.success("Datasource will be deleted soon.");
    },
  });
};

export default useDeleteDatasource;
