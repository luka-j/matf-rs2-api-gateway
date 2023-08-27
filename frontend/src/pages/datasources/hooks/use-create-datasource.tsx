import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "react-toastify";

import ccoConfigService, { ccoConfigKey } from "@/services/cco-config-service";

const useCreateDatasource = (isEditing?: boolean) => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ccoConfigService.createDatasource,
    onSuccess: () => {
      queryClient.invalidateQueries([ccoConfigKey]);
      toast.success(`Datasource will be ${isEditing ? "updated" : "created"} soon.`);
    },
  });
};

export default useCreateDatasource;
