import createApi from "./api-factory";

const configuratorApi = createApi({ commonPrefix: "conf" });

const getConfigs = async () => {
  return await configuratorApi.get("").then((res) => res.data);
};

const configuratorService = {
  getConfigs,
};

export default configuratorService;

export const configuratorKey = "configurator";
