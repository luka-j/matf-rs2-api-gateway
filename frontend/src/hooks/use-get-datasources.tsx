import { useQuery } from "@tanstack/react-query";

import ccoConfigService, { ccoConfigKey } from "@/services/cco-config-service";

const useGetDatasources = () => {
  return useQuery({
    queryKey: [ccoConfigKey],
    queryFn: () => ccoConfigService.getDatasources(),
  });
};

export default useGetDatasources;
