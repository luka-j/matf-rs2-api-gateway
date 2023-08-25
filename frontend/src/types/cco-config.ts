import { z } from "zod";

export const datasourceSchema = z.object({
  name: z.string(),
  type: z.string(),
  url: z.string(),
  username: z.string(),
  password: z.string(),
  connectionString: z.string(),
});

export const ccoConfigsSchema = z.object({
  title: z.string(),
  version: z.string(),
  databases: z.array(datasourceSchema),
  caches: z.array(datasourceSchema),
  queues: z.array(datasourceSchema),
});

export type Datasource = z.infer<typeof datasourceSchema>;

export type CCOConfigs = z.infer<typeof ccoConfigsSchema>;
