import { useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "react-toastify";

import configuratorService from "@/services/configurator-service";

const useUpdateConfigs = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: configuratorService.updateAllConfigs,
    onSuccess: () => {
      queryClient.invalidateQueries();
      toast.success("All configs will be updated soon.");
    },
  });
};

export default useUpdateConfigs;
