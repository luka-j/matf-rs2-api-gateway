import { z } from "zod";

export const rpConfigSchema = z.object({
  apiName: z.string(),
  apiVersion: z.string(),
});

export const rpsConfigSchema = z.array(rpConfigSchema);

export const rpDetailSchema = z.object({
  data: z.string(),
  validFrom: z.string(),
});

export type RpConfig = z.infer<typeof rpConfigSchema>;

export type RpsConfig = z.infer<typeof rpsConfigSchema>;

export type RpDetail = z.infer<typeof rpDetailSchema>;
