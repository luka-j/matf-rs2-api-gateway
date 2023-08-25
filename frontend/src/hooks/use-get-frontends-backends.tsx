import { useQuery } from "@tanstack/react-query";

import { FrontendBackend } from "@/types/api-configs";
import apiConfigService, { apiConfigKey } from "@/services/api-config-service";

const useGetFrontendsBackends = (configType: FrontendBackend) => {
  return useQuery({
    queryKey: [apiConfigKey, configType],
    queryFn: () => apiConfigService.getFrontendsBackends(configType),
  });
};

export default useGetFrontendsBackends;
