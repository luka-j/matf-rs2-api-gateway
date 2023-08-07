import { useState } from "react";

import { DataSource } from "@/mock/datasources";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Typography } from "@/components/ui/typography";

import EditDatasourceForm from "./edit-datasource-form";

const DatasourceCard = ({ datasource }: { datasource: DataSource }) => {
  const [isEditing, setIsEditing] = useState(false);

  return (
    <Card className="w-96">
      <CardHeader className="flex items-center justify-center">
        <CardTitle>{datasource.name}</CardTitle>
      </CardHeader>

      {isEditing ? (
        <EditDatasourceForm datasource={datasource} setIsEditing={setIsEditing} />
      ) : (
        <>
          <CardContent className="flex flex-col justify-center gap-4">
            <Typography variant="h4" className="text-center">
              {datasource.type}
            </Typography>
            <Typography variant="small" className="text-center">
              {datasource.url}
            </Typography>

            <Typography variant="large" className="text-center">
              {datasource.username}
            </Typography>
          </CardContent>

          <CardFooter className="flex flex-row items-center justify-end">
            <Button size="lg" variant="secondary" onClick={() => setIsEditing(true)}>
              Edit
            </Button>
          </CardFooter>
        </>
      )}
    </Card>
  );
};

export default DatasourceCard;
