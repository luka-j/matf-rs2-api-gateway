import { z } from "zod";

export type FrontendBackendDetailArgs = {
  configType: FrontendBackend;
  apiName: string;
  apiVersion: string;
};

export type FrontendBackend = "frontend" | "backend";

export const frontendBackendConfigSchema = z.object({
  apiName: z.string(),
  apiVersion: z.string(),
  basePath: z.string(),
});

export const frontendsBackendsConfigSchema = z.object({
  configs: z.array(frontendBackendConfigSchema),
});

export const frontendsBackendsDetailSchema = z.object({
  data: z.string(),
  validFrom: z.string(),
});

export type FrontendBackendConfig = z.infer<typeof frontendBackendConfigSchema>;

export type FrontendsBackendsConfig = z.infer<typeof frontendsBackendsConfigSchema>;
