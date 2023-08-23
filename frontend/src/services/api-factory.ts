import { useUserStore } from "@/stores/user-store";
import axios, { AxiosInstance, AxiosRequestConfig } from "axios";

const handleInterceptors = (apiInstance: AxiosInstance) => {
  apiInstance.defaults.headers.common["Content-Type"] = "application/json";

  apiInstance.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.data) {
        if (error.response.status === 401 || error.response.status === 403) {
          useUserStore.getState().logoutUser();
        }
      }
      return Promise.reject(error);
    },
  );

  apiInstance.interceptors.request.use(
    (config) => {
      const token = localStorage.getItem("token");

      if (token) config.headers["Authorization"] = `${token}`;

      return config;
    },
    (error) => Promise.reject(error),
  );
};

interface IApiOptions extends AxiosRequestConfig {
  commonPrefix: string;
}

const createApi = ({ commonPrefix, ...rest }: IApiOptions) => {
  const api = axios.create({
    baseURL: import.meta.env.DEV
      ? `http://localhost:5002/${commonPrefix}/`
      : `https://dashboard-rs2.luka-j.rocks/${commonPrefix}/`,
    ...rest,
  });

  handleInterceptors(api);

  return api;
};

export default createApi;
