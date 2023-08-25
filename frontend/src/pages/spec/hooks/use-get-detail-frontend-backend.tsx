import { useQuery } from "@tanstack/react-query";

import { FrontendBackendDetailArgs } from "@/types/api-configs";
import apiConfigService, { apiConfigKey } from "@/services/api-config-service";

const useGetDetailFrontendBackend = ({
  configType,
  apiName,
  apiVersion,
}: FrontendBackendDetailArgs) => {
  return useQuery({
    queryKey: [apiConfigKey, configType, apiName, apiVersion],
    queryFn: () => apiConfigService.getDetailFrontendBackend({ configType, apiName, apiVersion }),
    enabled: !!configType && !!apiName && !!apiVersion,
  });
};

export default useGetDetailFrontendBackend;
