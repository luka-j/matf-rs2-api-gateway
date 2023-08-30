import { z } from "zod";

export const addDatasourceSchema = z.object({
  name: z.string().min(1, "Name is required").min(5, "Name must be at least 5 characters"),
  type: z.string().min(1, "Type is required"),
  url: z.string().min(1, "URL is required"),
  password: z.string().min(1, "Password is required"),
  username: z.string().optional(),
  databaseName: z.string().optional(),
  port: z.coerce.number().optional(),
});

export type IAddDatasourceSchema = z.infer<typeof addDatasourceSchema>;
