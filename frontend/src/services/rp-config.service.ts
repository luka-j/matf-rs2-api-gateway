import { rpsConfigSchema } from "@/types/rp-configs";

import createApi from "./api-factory";

const rpConfigApi = createApi({ commonPrefix: "rp" });

const getFrontendsBackends = async () => {
  return await rpConfigApi.get("").then((res) => rpsConfigSchema.parse(res.data));
};

const rpConfigService = {
  getFrontendsBackends,
};

export default rpConfigService;

export const rpConfigKey = "rpConfig";
