import { useMutation, useQueryClient } from "@tanstack/react-query";

import apiConfigService, { apiConfigKey } from "@/services/api-config-service";

const useDeleteFrontend = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: apiConfigService.deleteFrontendBackend,
    onSuccess: () => {
      queryClient.invalidateQueries([apiConfigKey, "frontend"]);
    },
  });
};

export default useDeleteFrontend;
