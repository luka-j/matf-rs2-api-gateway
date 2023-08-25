import createApi from "./api-factory";

const configuratorApi = createApi({ commonPrefix: "conf" });

const getConfigs = async () => {
  return await configuratorApi.get("").then((res) => res.data);
};

const updateAllConfigs = async () => {
  return await configuratorApi.patch("update");
};

const configuratorService = {
  getConfigs,
  updateAllConfigs,
};

export default configuratorService;

export const configuratorKey = "configurator";
