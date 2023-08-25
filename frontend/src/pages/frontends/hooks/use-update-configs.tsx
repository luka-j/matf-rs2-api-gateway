import { useMutation, useQueryClient } from "@tanstack/react-query";

import configuratorService from "@/services/configurator-service";

const useUpdateConfigs = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: configuratorService.updateAllConfigs,
    onSuccess: () => {
      queryClient.invalidateQueries();
    },
  });
};

export default useUpdateConfigs;
