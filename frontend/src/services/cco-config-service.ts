import createApi from "./api-factory";

const ccoConfigApi = createApi({ commonPrefix: "cco" });

const getDatasources = async () => {
  return await ccoConfigApi.get("").then((res) => res.data);
};

const ccoConfigService = {
  getDatasources,
};

export default ccoConfigService;

export const ccoConfigKey = "ccoConfig";
