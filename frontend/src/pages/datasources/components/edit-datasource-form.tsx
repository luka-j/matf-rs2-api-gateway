import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2, Trash } from "lucide-react";
import { SubmitHandler, useForm } from "react-hook-form";

import { Datasource, DatasourceType } from "@/types/cco-config";
import { Button } from "@/components/ui/button";
import { CardContent, CardFooter } from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";

import useCreateDatasource from "../hooks/use-create-datasource";
import useDeleteDatasource from "../hooks/use-delete-datasource";
import { addDatasourceSchema, IAddDatasourceSchema } from "../schemas/add-datasource-schema";

interface IEditDatasourceFormProps {
  datasourceType: DatasourceType;
  datasource: Datasource;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}

const EditDatasourceForm = ({
  datasourceType,
  datasource,
  setIsEditing,
}: IEditDatasourceFormProps) => {
  const { mutate: deleteDatasource, isLoading: isDeleting } = useDeleteDatasource();

  const { mutate: createDatasource, isLoading } = useCreateDatasource(true);

  const form = useForm<IAddDatasourceSchema>({
    resolver: zodResolver(addDatasourceSchema),
    values: { ...datasource.datasource, name: datasource.title },
  });

  const onSubmit: SubmitHandler<IAddDatasourceSchema> = (data) => {
    createDatasource(
      {
        type: datasourceType,
        data: {
          datasource: data,
          title: data.name,
        },
      },
      { onSuccess: () => setIsEditing(false) },
    );
  };

  const onDelete = () => {
    deleteDatasource({ type: datasourceType, title: datasource.title });
    setIsEditing(false);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <CardContent className="grid grid-cols-1 gap-4 text-sm md:grid-cols-2">
          <FormField
            control={form.control}
            name="name"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Name</FormLabel>
                <FormControl>
                  <Input {...field} />
                </FormControl>
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
                  <Input {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="username"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Username</FormLabel>
                <FormControl>
                  <Input {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="password"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Password</FormLabel>
                <FormControl>
                  <Input type="password" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="url"
            render={({ field }) => (
              <FormItem className="col-span-1 md:col-span-2">
                <FormLabel>URL</FormLabel>
                <FormControl>
                  <Input {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="connectionString"
            render={({ field }) => (
              <FormItem className="col-span-1 md:col-span-2">
                <FormLabel>Connection string</FormLabel>
                <FormControl>
                  <Input {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </CardContent>

        <CardFooter className="flex items-center justify-between">
          <Button
            type="button"
            variant="destructive"
            size="icon"
            disabled={isDeleting}
            onClick={onDelete}
          >
            {isDeleting ? <Loader2 className="animate-spin" /> : <Trash />}
          </Button>
          <div className="flex items-center gap-2">
            <Button type="button" variant="secondary" onClick={() => setIsEditing(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading ? <Loader2 className="animate-spin" /> : "Save"}
            </Button>
          </div>
        </CardFooter>
      </form>
    </Form>
  );
};

export default EditDatasourceForm;
