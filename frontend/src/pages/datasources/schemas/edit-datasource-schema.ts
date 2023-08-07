import { z } from "zod";

import { addDatasourceSchema } from "./add-datasource-schema";

export const editDatasourceSchema = addDatasourceSchema.extend({
  password: z.string().min(8, "Password must be at least 8 characters long"),
});

export type IEditDatasourceSchema = z.infer<typeof editDatasourceSchema>;
