import { zodResolver } from "@hookform/resolvers/zod";
import { Trash } from "lucide-react";
import { SubmitHandler, useForm } from "react-hook-form";

import { DataSource } from "@/mock/datasources";
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

import { editDatasourceSchema, IEditDatasourceSchema } from "../schemas/edit-datasource-schema";

interface IEditDatasourceFormProps {
  datasource: DataSource;
  setIsEditing: React.Dispatch<React.SetStateAction<boolean>>;
}

const EditDatasourceForm = ({ datasource, setIsEditing }: IEditDatasourceFormProps) => {
  const form = useForm<IEditDatasourceSchema>({
    resolver: zodResolver(editDatasourceSchema),
    defaultValues: { ...datasource, password: "" },
  });

  const onSubmit: SubmitHandler<IEditDatasourceSchema> = (data) => {
    console.log(data);
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
            name="url"
            render={({ field }) => (
              <FormItem>
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
            name="username"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Username</FormLabel>
                <FormControl>
                  <Input placeholder="tom12345@admin" {...field} />
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
        </CardContent>

        <CardFooter className="flex items-center justify-between">
          <Button type="button" variant="destructive" size="icon">
            <Trash />
          </Button>
          <div className="flex items-center gap-2">
            <Button type="button" variant="secondary" onClick={() => setIsEditing(false)}>
              Cancel
            </Button>
            <Button type="submit">Save</Button>
          </div>
        </CardFooter>
      </form>
    </Form>
  );
};

export default EditDatasourceForm;
