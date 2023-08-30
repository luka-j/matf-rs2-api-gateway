import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { SubmitHandler, useForm } from "react-hook-form";

import { DatasourceType } from "@/types/cco-config";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";

import useCreateDatasource from "../hooks/use-create-datasource";
import { addDatasourceSchema, IAddDatasourceSchema } from "../schemas/add-datasource-schema";

interface IAddDatasourceFormProps {
  setOpenDialog: React.Dispatch<React.SetStateAction<boolean>>;
  datasourceType: string;
}

const AddDatasourceForm = ({ setOpenDialog, datasourceType }: IAddDatasourceFormProps) => {
  const { mutate: createDatasource, isLoading } = useCreateDatasource();

  const form = useForm<IAddDatasourceSchema>({
    resolver: zodResolver(addDatasourceSchema),
    defaultValues: {
      name: "",
      type: "",
      username: "",
      password: "",
      url: "",
      databaseName: "",
    },
  });

  const onSubmit: SubmitHandler<IAddDatasourceSchema> = (data) => {
    createDatasource(
      {
        type: (datasourceType + "s") as DatasourceType,
        data: {
          datasource: data,
          title: data.name,
        },
      },
      { onSuccess: () => setOpenDialog(false) },
    );
  };

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className="grid grid-cols-1 gap-4 md:grid-cols-2"
      >
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Name</FormLabel>
              <FormControl>
                <Input placeholder={`${datasourceType}1`} {...field} className="w-full" />
              </FormControl>
              <FormDescription>This is the name of your {datasourceType}.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="type"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Type</FormLabel>
              <FormControl>
                <Input placeholder={`${datasourceType} type 1`} {...field} />
              </FormControl>
              <FormDescription>This is the type of your {datasourceType}.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        {datasourceType !== "cache" && (
          <FormField
            control={form.control}
            name="username"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Username</FormLabel>
                <FormControl>
                  <Input placeholder="tom12345@admin" {...field} />
                </FormControl>
                <FormDescription>
                  This is the username you will use to connect to your {datasourceType}.
                </FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
        )}
        <FormField
          control={form.control}
          name="password"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Password</FormLabel>
              <FormControl>
                <Input placeholder="tom12345@admin" type="password" {...field} />
              </FormControl>
              <FormDescription>
                This is the password you will use to connect to your {datasourceType}.
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={form.control}
          name="url"
          render={({ field }) => (
            <FormItem>
              <FormLabel>URL</FormLabel>
              <FormControl>
                <Input placeholder={`${datasourceType}://localhost:10000`} {...field} />
              </FormControl>
              <FormDescription>This is the URL to your {datasourceType}.</FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
        {datasourceType === "database" && (
          <>
            <FormField
              control={form.control}
              name="port"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Port</FormLabel>
                  <FormControl>
                    <Input placeholder={`54321`} {...field} />
                  </FormControl>
                  <FormDescription>This is the port of your {datasourceType}.</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="databaseName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Database name</FormLabel>
                  <FormControl>
                    <Input placeholder={`database name`} {...field} />
                  </FormControl>
                  <FormDescription>This is the name of your {datasourceType}.</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />
          </>
        )}
        <Button type="submit" className="col-span-1 w-full md:col-span-2" disabled={isLoading}>
          {isLoading ? <Loader2 className="animate-spin" /> : `Add ${datasourceType}`}
        </Button>
      </form>
    </Form>
  );
};

export default AddDatasourceForm;
