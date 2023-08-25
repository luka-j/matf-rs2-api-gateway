import { z } from "zod";

export const rpsConfigSchema = z.object({
  configs: z.array(
    z.object({
      apiName: z.string(),
      apiVersion: z.string(),
      basePath: z.string(),
    }),
  ),
});

export type RpsConfig = z.infer<typeof rpsConfigSchema>;
